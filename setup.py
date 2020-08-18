import setuptools

with open("README.md", "r") as fh:
    long_description = fh.read()

setuptools.setup(
    name="ladybug-grasshopper-dotnet",
    use_scm_version=True,
    setup_requires=['setuptools_scm'],
    author="Ladybug Tools",
    author_email="info@ladybug.tools",
    description="GHA files for the Ladybug Grasshopper Plugin.",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/ladybug-tools/ladybug-grasshopper-dotnet",
    packages=setuptools.find_packages(),
    include_package_data=True,
    install_requires=[],
    classifiers=[
        "License :: OSI Approved :: GNU General Public License v3 (GPLv3)",
        "Operating System :: OS Independent"
    ],
)
